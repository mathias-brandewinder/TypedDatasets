namespace Datasets

type Example<'Observation,'Label> = {
    Observation: 'Observation
    Label: 'Label
    }

type Dataset<'T> =
    abstract member Read: unit -> seq<'T>
    abstract member Source: string